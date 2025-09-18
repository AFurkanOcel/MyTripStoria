<template>
    <h1>Hello World!</h1>
    <pre>
        {{ data }}
    </pre>
</template>

<script lang="ts" setup>
definePageMeta({
    layout: false,
});

const data = await useFetch("");
</script>