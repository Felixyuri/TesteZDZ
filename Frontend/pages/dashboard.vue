<template>
  <v-container fluid>
    <v-row>
      <v-col cols="12">
        <h1>Dashboard</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12" md="6">
        <v-card>
          <v-card-title>
            <h2>Produtos</h2>
          </v-card-title>
          <v-card-text>
            <v-data-table
              :headers="headersProducts"
              :items="processedProducts"
            >
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" md="6">
        <v-card>
          <v-card-title>
            <h2>Fornecedores</h2>
          </v-card-title>
          <v-card-text>
            <v-data-table
              :headers="headersSuppliers"
              :items="suppliers"
            >
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
  name: 'dashboard',

  data() {
    return {
      headersProducts: [
        { text: 'Nome', value: 'name' },
        { text: 'Quantidade', value: 'quantity' },
        { text: 'Fornecedores', value: 'suppliers' }
      ],
      headersSuppliers: [
        { text: 'Nome', value: 'name'},
        { text: 'Produtos em estoque', value: 'productsStock'},
        { text: 'Produtos registrados', value: 'registeredProducts'}
      ],
      products: [],
      suppliers: []
    }
  },

  computed: {
    processedProducts() {
      return this.products.map(product => {
        const suppliers = product.suppliers.$values.map(supplierId => this.getSupplierName(supplierId));
        return { ...product, suppliers };
      });
    }
  },

  created() {
    this.fetchSuppliers();
    this.fetchProducts();
  },

  methods: {
    async fetchSuppliers() {
      try {
        let response = await this.$axios({
          method: 'GET',
          url: 'https://localhost:7006/Supplier'
        });
        this.suppliers = response.data.$values;
      } catch (error) {
        console.error('Erro ao buscar fornecedores:', error);
      }
    },

    async fetchProducts() {
      try {
        let response = await this.$axios({
          method: 'GET',
          url: 'https://localhost:7006/Product'
        });
        this.products = response.data.$values;
      } catch (error) {
        console.error('Erro ao buscar produtos:', error);
      }
    },

    getSupplierName(supplierId) {
      const supplier = this.suppliers.find(s => s.id === supplierId);
      return supplier ? supplier.name : '';
    },
  }
}
</script>

<style scoped>
.v-card {
  margin-bottom: 20px;
}
</style>
