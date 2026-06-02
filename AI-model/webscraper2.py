import requests
import time

from bs4 import BeautifulSoup

from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.common.exceptions import StaleElementReferenceException
from selenium.webdriver.edge.options import Options
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

# ========== Edge browser instellingen ==========
options = Options()
options.add_argument("--start-maximized")  # Start browser gemaximaliseerd
options.add_argument("--disable-blink-features=AutomationControlled")  # Anti-bot detectie verminderen

# User-Agent header om echte browser te simuleren
headers = {
    "user-agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36 Edg/128.0.0.0",
}

# Start Edge WebDriver
driver = webdriver.Edge(options=options)

# Productcategorie URL (Albert Heijn schoonmaakmiddelen)
url = "https://www.ah.nl/producten/1498/schoonmaakmiddelen"

# Open pagina
driver.get(url)

# Cookie banner accepteren
btn = WebDriverWait(driver, 5).until(
    EC.element_to_be_clickable((By.CSS_SELECTOR, '[data-testid="accept-cookies"]'))
)

btn.click()


def get_image(soup, url):
    """
    Haalt productafbeelding uit HTML van productpagina
    """

    # Controleer of er een enkele afbeelding container is
    div = soup.find_all("div", {
        "class": "image-carousel_root__x2uxe image-carousel_singleImageContainer__wDkHN"
    })

    if div:
        # Enkele afbeelding
        img = div[0].find("img", {"data-testid": "image-carousel-display-image"})
    else:
        # Meerdere afbeeldingen (carousel)
        count_button = soup.find_all("div", {
            "class": "image-carousel_thumbnails__Gc_Mz"
        })[0]

        div = soup.find_all("div", {
            "class": "image-carousel_imageSlide__fAvOO"
        })

        # Kies middelste afbeelding uit carousel
        img = div[int(len(count_button) / 2)].find(
            "img",
            {"data-testid": "image-carousel-display-image"}
        )

    # Controleer of afbeelding bestaat
    if not img:
        raise Exception(f"Afbeelding niet gevonden | link: {url}")

    # Haal image URL op
    link_image = img.get("src")

    # Verhoog resolutie van afbeelding
    link_image = link_image.replace("400x400", "800x800")
    link_image = link_image.replace("Q85", "Q90")

    print(link_image)

    # Extra check
    if not link_image:
        raise Exception(f"Lege image src | link: {url}")

    return link_image


def get_ImageLink(link):
    """
    Bezoekt productpagina en haalt titel + afbeelding op
    """

    # HTTP request naar productpagina
    res = requests.get(link, headers=headers, timeout=10)

    if res.status_code != 200:
        raise Exception(f"Request mislukt: {res.status_code} | link: {link}")

    # Parse HTML
    soup = BeautifulSoup(res.text, "html.parser")

    # Producttitel ophalen
    title = soup.find("h1").text
    print(title)

    # Afbeelding ophalen
    link_image = get_image(soup, link)

    return title, link_image


def download_image(title, link_image):
    """
    Download afbeelding en sla op in lokale map
    """

    res = requests.get(link_image)

    # Bestand opslaan
    with open(f"image/{title}.jpg", "wb") as f:
        f.write(res.content)


# ========== Hoofd loop ==========
while True:

    # Zoek alle productkaarten op de pagina
    products = driver.find_elements(
        By.CSS_SELECTOR,
        '[data-testid="product-card-vertical-container"]'
    )

    for product in products:
        # Haal productlink op
        link = product.find_element(By.TAG_NAME, "a").get_attribute("href")

        # Haal titel + afbeelding
        title, link_image = get_ImageLink(link)

        # Download afbeelding
        download_image(title, link_image)

        # Kleine vertraging om blokkering te voorkomen
        time.sleep(1)

    # ===== Paginering =====
    try:
        # Zoek "volgende pagina" knop
        next_btn = WebDriverWait(driver, 5).until(
            EC.element_to_be_clickable(
                (By.CSS_SELECTOR, 'button[aria-label="Go to next page"]')
            )
        )

        # Scroll naar knop
        driver.execute_script("arguments[0].scrollIntoView();", next_btn)
        time.sleep(1)

        # Klik volgende pagina
        next_btn.click()

        # Wacht tot oude producten verdwijnen (pagina refresh detectie)
        WebDriverWait(driver, 10).until(
            EC.staleness_of(products[0])
        )

        time.sleep(2)

    except:
        print("✅ laatste pagina bereikt")
        break

# Sluit browser
driver.quit()