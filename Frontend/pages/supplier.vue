<template>
  <v-data-table
    :headers="headersSuppliers"
    :items="suppliers"
  >
    <template v-slot:top>
      <v-toolbar
        flat
      >
        <v-toolbar-title>Fornecedores</v-toolbar-title>
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
              @click="addNewSupplier"
            >
              Novo fornecedor
            </v-btn>
          </template>
        <v-dialog
          v-model="dialog"
          max-width="500px"
        >
          <v-card>
            <v-form ref="form">
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
                        v-model="supplier.name"
                        :rules="[v => !!v || 'Insira um nome']"
                        required
                        label="Nome"
                      ></v-text-field>
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
            </v-form>
          </v-card>
        </v-dialog>
        <v-dialog v-model="dialogDelete" max-width="500px">
          <v-card>
            <v-card-title class="text-h5">Tem certeza que deseja deletar esse fornecedor?</v-card-title>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue-darken-1" variant="text" @click="closeDelete">Cancelar</v-btn>
              <v-btn color="blue-darken-1" variant="text" @click="deleteSupplierConfirm">Deletar</v-btn>
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
        @click="editSupplier(item)"
      >
        mdi-pencil
      </v-icon>
      <v-icon
        size="small"
        @click="deleteSupplier(item)"
      >
        mdi-delete
      </v-icon>
    </template>
  </v-data-table>
</template>

<script>
export default {
  name: 'supplier',
  
  data() {
    return {
      dialog: false,
      dialogDelete: false,
      headersSuppliers: [
        { text: 'Nome', value: 'name'},
        { text: 'Produtos em estoque', value: 'productsStock'},
        { text: 'Produtos registrados', value: 'registeredProducts'},
        { text: '', sortable: false, value: 'actions' }
      ],
      suppliers: [],
      editedIndex: -1,
      supplier: { name: '' },
      defaultItem: { name: '' },
    };
  },

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? 'Novo fornecedor' : 'Editar Fornecedor';
    },
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

    getSupplierName(supplierId) {
      const supplier = this.suppliers.find(s => s.value === supplierId);
      return supplier ? supplier.text : '';
    },

    editSupplier(item) {
      this.editedIndex = item.id;
      this.supplier = {...item};

      this.dialog = true;
    },

    addNewSupplier() {
      this.supplier = Object.assign({}, this.defaultItem);
      this.dialog = true;
    },

    deleteSupplier(item) {
      this.editedIndex = item.id;
      this.supplier = Object.assign({}, item);
      this.dialogDelete = true;
    },

    async deleteSupplierConfirm() {
      try {
        await this.$axios({
          method: 'DELETE',
          url: `https://localhost:7006/Supplier/${this.editedIndex}`
        });
        await this.fetchSuppliers();
        this.closeDelete();
      } catch (error) {
        console.error('Erro ao deletar fornecedor:', error);
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
        this.supplier = Object.assign({}, this.defaultItem)
        this.editedIndex = -1
      })
    },

    async save () {
      const isValid = await this.$refs.form.validate();
      if (!isValid) {
        return;
      }

      if (this.editedIndex == -1) {
        try {
          await this.$axios({
            method: 'POST',
            url: 'https://localhost:7006/Supplier',
            data: this.supplier
          });
          await this.fetchSuppliers();
          this.supplier = { name: '' };
        } catch (error) {
          console.error('Erro ao enviar fornecedor:', error);
        }
      } else {
        try {
          delete this.supplier.products;

          await this.$axios({
            method: 'PUT',
            url: `https://localhost:7006/Supplier/${this.editedIndex}`,
            data: this.supplier
          });
          await this.fetchSuppliers();
        } catch (error) {
          console.error('Erro ao enviar fornecedor:', error);
        }
      }
      this.close();
    },
  },
}
</script>
