<template>
    <img src="assets/images/angel.png" style="width: 200px; height: auto; border: 5px solid black; border-radius: 10px;"/>
    <h1>Hello Deneme!</h1>
    <button1 @click = count++ />
    {{ count }}
    <h2>Thanks!</h2>
</template>

<script lang="ts" setup>
import "/assets/css/main.css"
const count = ref(0);
definePageMeta({
    layout: false,
});
</script>

<style module>
h2{
    color: yellow;
}
</style>