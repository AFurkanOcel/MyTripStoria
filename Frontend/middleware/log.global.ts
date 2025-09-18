export default defineNuxtRouteMiddleware((to, from) => {
    console.log("log middleware", to, from);
});