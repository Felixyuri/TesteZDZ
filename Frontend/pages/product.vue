<template>
  <v-data-table
    :headers="headers"
    :items="processedProducts"
  >
    <template v-slot:top>
      <v-toolbar
        flat
      >
        <v-toolbar-title>Produtos</v-toolbar-title>
        <v-divider
          class="mx-4"
          inset
          vertical
        ></v-divider>
        <v-spacer></v-spacer>
        <template>
            <v-btn
              class="mb-2"
              color="primary"
              dark
              @click="addNewProduct"
            >
              Novo Produto
            </v-btn>
          </template>
        <v-dialog
          v-model="dialog"
          max-width="500px"
        >
          <v-card>
            <v-card-title>
              <span class="text-h5">{{ formTitle }}</span>
            </v-card-title>

            <v-card-text>
              <v-container>
                <v-row>
                  <v-col
                    cols="12"
                    md="4"
                    sm="6"
                  >
                    <v-text-field
                      v-model="product.name"
                      :rules="[v => !!v || 'Insira um nome']"
                      required
                      label="Nome"
                    ></v-text-field>
                  </v-col>
                  <v-col
                    cols="12"
                    md="4"
                    sm="6"
                  >
                    <v-text-field
                      v-model="product.quantity"
                      type="number"
                      :rules="[v => !!v || 'Insira uma quantidade']"
                      label="Quantidade"
                      required
                    ></v-text-field>
                  </v-col>
                  <v-col
                    cols="12"
                    md="4"
                    sm="6"
                  >
                    <v-select
                      v-model="product.suppliers"
                      :items="suppliers"
                      label="Fornecedor"
                      :rules="[v => v && v.length > 0 || 'Insira um fornecedor']"
                      multiple
                      required
                    ></v-select>
                  </v-col>
                </v-row>
              </v-container>
            </v-card-text>

            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn
                color="blue-darken-1"
                variant="text"
                @click="close"
              >
                Cancelar
              </v-btn>
              <v-btn
                color="blue-darken-1"
                variant="text"
                @click="save"
              >
                Salvar
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <v-dialog v-model="dialogDelete" max-width="500px">
          <v-card>
            <v-card-title class="text-h5">Tem certeza que deseja deletar esse produto?</v-card-title>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue-darken-1" variant="text" @click="closeDelete">Cancelar</v-btn>
              <v-btn color="blue-darken-1" variant="text" @click="deleteProductConfirm">Deletar</v-btn>
              <v-spacer></v-spacer>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-toolbar>
    </template>
    <template v-slot:item.actions="{ item }">
      <v-icon
        class="me-2"
        size="small"
        @click="editProduct(item)"
      >
        mdi-pencil
      </v-icon>
      <v-icon
        size="small"
        @click="deleteProduct(item)"
      >
        mdi-delete
      </v-icon>
    </template>
  </v-data-table>
</template>

<script>
export default {
  name: 'product',

  data() {
    return {
      dialog: false,
      dialogDelete: false,
      headers: [
        { text: 'Nome', align: 'left', sortable: false, value: 'name' },
        { text: 'Quantidade', value: 'quantity' },
        { text: 'Fornecedores', value: 'suppliers' },
        { text: '', sortable: false, value: 'actions' }
      ],
      products: [],
      suppliers: [],
      editedIndex: -1,
      product: {
        name: '',
        quantity: 0,
        suppliers: []
      },
      defaultItem: {
        name: '',
        quantity: 0,
        suppliers: []
      },
    };
  },

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? 'Novo produto' : 'Editar Produto';
    },

    processedProducts() {
      return this.products.map(product => {
        const suppliers = product.suppliers.$values.map(supplierId => this.getSupplierName(supplierId));
        return { ...product, suppliers };
      });
    }
  },

  watch: {
    dialog(val) {
      val || this.close()
    },

    dialogDelete(val) {
      val || this.closeDelete()
    },
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
        this.suppliers = response.data.$values.map(supplier => ({
          text: supplier.name,
          value: supplier.id
        }));
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
      const supplier = this.suppliers.find(s => s.value === supplierId);
      return supplier ? supplier.text : '';
    },

    editProduct(item) {
      this.editedIndex = item.id;
      this.product = {...item};


      this.product.suppliers = this.product.suppliers.map(supplierName => {
        return this.suppliers.find(supplier => supplier.text === supplierName);
      });

      this.dialog = true;
    },

    addNewProduct() {
      this.product = Object.assign({}, this.defaultItem);
      this.dialog = true;
    },

    deleteProduct(item) {
      this.editedIndex = item.id;
      this.product = Object.assign({}, item);
      this.dialogDelete = true;
    },

    async deleteProductConfirm() {
      try {
        await this.$axios({
          method: 'DELETE',
          url: `https://localhost:7006/Product/${this.editedIndex}`
        });
        await this.fetchProducts();
        this.closeDelete();
      } catch (error) {
        console.error('Erro ao deletar produto:', error);
      }
    },

    close() {
      this.dialog = false
      this.$nextTick(() => {
        this.editedIndex = -1
      })
    },

    closeDelete () {
      this.dialogDelete = false
      this.$nextTick(() => {
        this.product = Object.assign({}, this.defaultItem)
        this.editedIndex = -1
      })
    },

    async save () {
      if (this.editedIndex == -1) {
        try {
          await this.$axios({
            method: 'POST',
            url: 'https://localhost:7006/Product',
            data: this.product
          });
          await this.fetchProducts();
          close();
          this.product = {
            name: '',
            suppliers: [],
            quantity: 0
          };
        } catch (error) {
          console.error('Erro ao enviar produto:', error);
        }

      } else {
        try {
          var suppliersIds = [];

          this.product.suppliers.forEach(s => {
            if(typeof s == 'string') {
              var productSuppliers = this.suppliers.find(item => item.text == s);

              if(productSuppliers) {
                var supplierValue = productSuppliers.value;
                suppliersIds.push(supplierValue);
              } else {
                console.log('Fornecedor não encontrado:', s);
              }
            }
          });

          if(suppliersIds.length) {
            this.product.suppliers = suppliersIds
          }

          await this.$axios({
            method: 'PUT',
            url: `https://localhost:7006/Product/${this.editedIndex}`,
            data: {...this.product, Id: this.product.id }
          });
          await this.fetchProducts();
          close();
        } catch (error) {
          console.error('Erro ao enviar produto:', error);
        }

      }
      this.close()
    },
  },
}
</script>
