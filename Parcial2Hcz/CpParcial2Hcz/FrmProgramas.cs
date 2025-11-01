using CadParcial2Hcz;
using ClnParcial2Hcz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpParcial2Hcz
{
    public partial class FrmProgramas : Form
    {
        private bool esNuevo = false;
        public FrmProgramas()
        {
            InitializeComponent();
        }
        private void listar()
        {
            var lista = ProgramaCln.listarPa(txtParametro.Text.Trim());
            dgvLista.DataSource = lista;

            dgvLista.Columns["id"].Visible = false;                     //ESTO NO SE MUESTRA
            dgvLista.Columns["idCanal"].Visible = false;
            dgvLista.Columns["estado"].HeaderText="Estado";
           // dgvLista.Columns["estadoRegistro"].Visible = false;

            dgvLista.Columns["titulo"].HeaderText = "Título";                   // ESTO SE MUESTRA
            dgvLista.Columns["descripcion"].HeaderText = "Descripción";
            dgvLista.Columns["duracion"].HeaderText = "Duración (min)";
            dgvLista.Columns["productor"].HeaderText = "Productor";
            dgvLista.Columns["fechaEstreno"].HeaderText = "Fecha Estreno";
            dgvLista.Columns["usuarioRegistro"].HeaderText = "Usuario Registro";
            dgvLista.Columns["fechaRegistro"].HeaderText = "Fecha Registro";

            dgvLista.Columns["Canal"].HeaderText = "Canal";

            if (lista.Count > 0) dgvLista.CurrentCell = dgvLista.Rows[0].Cells["titulo"];
            btnEditar.Enabled = lista.Count > 0;
            btnEliminar.Enabled = lista.Count > 0;
        }

        private void cargarCanal()
        {
            var lista = CanalCln.listar();
            cbxCanal.DataSource = lista;
            cbxCanal.ValueMember = "id";
            cbxCanal.DisplayMember = "nombre"; // o descripcion según tu BD
            cbxCanal.SelectedIndex = -1;
        }

        private void FrmProgramas_Load(object sender, EventArgs e)
        {
            Size = new Size(834, 353);
            listar();
            cargarCanal();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            esNuevo = true;
            pnlAcciones.Enabled = false;
            Size = new Size(834, 526);
            txtTitulo.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            esNuevo = false;
            pnlAcciones.Enabled = false;
            Size = new Size(834, 526);

            int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
            var programa = ProgramaCln.obtenerUno(id);

            cbxCanal.SelectedValue = programa.idCanal;
            txtTitulo.Text = programa.titulo;
            txtDescripcion.Text = programa.descripcion;
            nudDuracion.Value = programa.duracion;
            txtProductor.Text = programa.productor;
            dtpFechaEstreno.Value = programa.fechaEstreno;

            txtTitulo.Focus();
        }

        private void limpiar()
        {
            txtTitulo.Clear();
            txtDescripcion.Clear();
            cbxCanal.SelectedIndex = -1;
            nudDuracion.Value = 30;
            txtProductor.Clear();
            dtpFechaEstreno.Value = DateTime.Now;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Size = new Size(834, 353);
            pnlAcciones.Enabled = true;
            limpiar();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            listar();
        }
        private void txtParametro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) listar();
        }
        private bool validar()
        {
            bool esValido = true;

            erpTitulo.Clear();
            erpCanal.Clear();
            erpDescripcion.Clear();
            erpDuracion.Clear();
            erpProductor.Clear();
            erpFechaEstreno.Clear();

            if (string.IsNullOrEmpty(txtTitulo.Text))
            {
                erpTitulo.SetError(txtTitulo, "El título es obligatorio");
                esValido = false;
            }

            if (cbxCanal.SelectedIndex == -1)
            {
                erpCanal.SetError(cbxCanal, "Debe seleccionar un canal");
                esValido = false;
            }

            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                erpDescripcion.SetError(txtDescripcion, "La descripción es obligatoria");
                esValido = false;
            }

            if (nudDuracion.Value == 0)
            {
                erpDuracion.SetError(nudDuracion, "La duración debe ser mayor a cero");
                esValido = false;
            }

            if (string.IsNullOrEmpty(txtProductor.Text))
            {
                erpProductor.SetError(txtProductor, "El productor es obligatorio");
                esValido = false;
            }

            if (dtpFechaEstreno.Value == null)
            {
                erpFechaEstreno.SetError(dtpFechaEstreno, "Debe seleccionar fecha de estreno");
                esValido = false;
            }

            return esValido;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (validar())
            {
                var programa = new Programa();
                programa.titulo = txtTitulo.Text.Trim();
                programa.descripcion = txtDescripcion.Text.Trim();
                programa.idCanal = (int)cbxCanal.SelectedValue;
                programa.duracion = (int)nudDuracion.Value;
                programa.productor = txtProductor.Text.Trim();
                programa.fechaEstreno = dtpFechaEstreno.Value;
                programa.usuarioRegistro = "admin";

                if (esNuevo)
                {
                    programa.fechaRegistro = DateTime.Now;
                    programa.estado = 1;
                    ProgramaCln.insertar(programa);
                }
                else
                {
                    programa.id = (int)dgvLista.CurrentRow.Cells["id"].Value;
                    ProgramaCln.actualizar(programa);
                }

                listar();
                btnCancelar.PerformClick();

                MessageBox.Show("Programa guardado correctamente",
                    "::: Mensaje - Sistema :::",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un programa para eliminar",
                    "::: Mensaje - Sistema :::", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
            string titulo = dgvLista.CurrentRow.Cells["titulo"].Value.ToString();

            DialogResult dialog = MessageBox.Show(
                $"¿Está seguro de eliminar el programa '{titulo}'?",
                "::: Mensaje - Sistema :::",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dialog == DialogResult.Yes)
            {
                // Usuario fijo o futuro usuario del sistema
                string usuario = "admin";

                ProgramaCln.eliminar(id, usuario);

                listar(); // refrescar grilla

                MessageBox.Show(
                    "Programa dado de baja correctamente",
                    "::: Mensaje - Sistema :::",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }


        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
