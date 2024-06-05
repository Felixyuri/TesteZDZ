<template>
  <v-app dark>
    <v-app-bar :clipped-left="clipped" fixed app>
      <NuxtLink :to="{ path: '/dashboard'}">
        <v-btn>
          Dashboard
        </v-btn>
      </NuxtLink>
      <NuxtLink :to="{ path: '/product'}">
        <v-btn>
          Produtos
        </v-btn>
      </NuxtLink>
      <NuxtLink :to="{ path: '/supplier'}">
        <v-btn>
          Fornecedores
        </v-btn>
      </NuxtLink>
      <v-spacer />
    </v-app-bar>
    <v-main>
      <v-container>
        <Nuxt />
      </v-container>
    </v-main>
    <v-footer :absolute="!fixed" app>
      <span>&copy; {{ new Date().getFullYear() }}</span>
    </v-footer>
  </v-app>
</template>

<script>
export default {
  name: 'DefaultLayout',
  data() {
    return {
      clipped: false,
      drawer: false,
      fixed: false,
      miniVariant: false,
      right: true,
      rightDrawer: false,
    }
  }
}
</script>
