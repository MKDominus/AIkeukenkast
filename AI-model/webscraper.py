import os
import requests
from bs4 import BeautifulSoup

# URL van Albert Heijn schoonmaakmiddelen categorie
url = r'https://www.ah.nl/producten/1498/schoonmaakmiddelen'

# Headers om een echte browser te simuleren (anti-bot bescherming omzeilen)
headers = {
    "user-agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36 Edg/128.0.0.0",
}

# Eerste request naar de categoriepagina
response = requests.get(url, headers=headers)

# Startpagina nummer voor paginering
pagenum = 1


def get_image(soup, url):
    """
    Haalt de productafbeelding uit de productpagina HTML
    """

    # Probeer standaard carousel container te vinden
    div = soup.find_all("div", {
        "class": "image-carousel_root__x2uxe image-carousel_singleImageContainer__wDkHN"
    })

    if div:
        # Als er een enkele afbeelding is
        img = div[0].find("img", {"data-testid": "image-carousel-display-image"})
    else:
        # Anders: meerdere thumbnails/carousel slides
        count_button = soup.find_all("div", {
            "class": "image-carousel_thumbnails__Gc_Mz"
        })[0]

        div = soup.find_all("div", {
            "class": "image-carousel_imageSlide__fAvOO"
        })

        # Kies middelste afbeelding uit de lijst
        img = div[int(len(count_button) / 2)].find(
            "img",
            {"data-testid": "image-carousel-display-image"}
        )

    # Controle of afbeelding gevonden is
    if not img:
        raise Exception(f"Image tag niet gevonden | link: {url}")

    # Haal image URL op
    link_image = img.get("src")

    # Verhoog resolutie van afbeelding
    link_image = link_image.replace("400x400", "800x800")
    link_image = link_image.replace("Q85", "Q90")

    print(link_image)

    return link_image


def get_ImageLink(link):
    """
    Bezoekt een productpagina en haalt titel + afbeelding op
    """

    # Bouw volledige URL
    url = "https://www.ah.nl" + link

    # HTTP request met timeout
    res = requests.get(url, headers=headers, timeout=10)

    # Controle op fout
    if res.status_code != 200:
        raise Exception(f"Request mislukt: {res.status_code} | link: {url}")

    # HTML parser
    soup = BeautifulSoup(res.text, "html.parser")

    # Producttitel ophalen
    title = soup.find("h1").text
    print(title)

    # Afbeelding ophalen
    link_image = get_image(soup, url)

    return title, link_image


def download_image(title, link_image):
    """
    Downloadt afbeelding en slaat deze lokaal op
    """

    # Maak map aan als die niet bestaat
    os.makedirs("image", exist_ok=True)

    # Download afbeelding
    res = requests.get(link_image)

    # Sla bestand op als JPG
    with open(f"image/{title}.jpg", "wb") as f:
        f.write(res.content)


# Loop door alle productpagina's (pagination)
while True:
    page = url + f"?page={pagenum}"
    print(page)

    # Request naar categoriepagina
    res = requests.get(page, headers=headers)

    if res.status_code != 200:
        raise Exception(f"Request mislukt: {res.status_code} | link: {page}")

    # HTML parser
    soup = BeautifulSoup(res.text, "html.parser")

    # Zoek product grid container
    product_results = soup.find_all(
        "div",
        {"class": "product-results_productGrid__R6mdr product-results_default__d57JR"}
    )

    # Haal alle product wrappers eruit
    products = product_results[0].find_all(
        "div",
        {"class": "product-results_productWrapper__2J9wI"}
    )

    # Als er producten zijn
    if products:
        for product in products:
            # Zoek link naar productpagina
            a = product.find("a")
            link = a.get("href")

            # Haal titel + afbeelding op
            title, link_image = get_ImageLink(link)

            # Download afbeelding
            download_image(title, link_image)

        # Ga naar volgende pagina
        pagenum += 1

    else:
        print("Er is geen nieuwe pagina meer")
        break




