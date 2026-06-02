<script lang="ts">

// here go the imports 

/* 
    component props are variables that can be used or changed by the parent of the component.
    if you pass a object in the parameters of the variable it becomes an event. see the below examples (without typescript).
    also see official documentation: https://svelte.dev/docs/svelte/$props
*/


let {
    // these are normal props. variable1 for example has a default value of "hello". this can be changed by the parent.
    variable1 = "hello",

    // these are events. the event can be called by DOM events such as "onclick". 
    // see docs: https://svelte.dev/docs/svelte/basic-markup#Events
    event1
} = $props()

// other functions if needed

</script>

<!-- 
    Here goes the basic html elements. Dont use elements such as <head> and <body> 
    to avoid unexpected styling behaviors.  
-->

<!-- example button with a normal prop and event -->
 <button onclick={event1("event1 has fired")}>{variable1}</button>
<!-- 
    You have to catch the event at the parent in order for the event to work.
    You can use unnamed functions or named functions to handle your event.
    This is how that would look like in the parent page:

    <script lang="ts">
        import exampleComponent from "$lib/components/exampleComponent"

        function event1Handler(eventOutput){
            console.log(eventOutput)
            // output in the console will be "event1 has fired"
        }
    </script>


    <exampleComponent event1={event1Handler}></exampleComponent>
-->
 

<!-- Here goes the styling of the component. -->