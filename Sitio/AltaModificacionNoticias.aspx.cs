﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using RefServicio;

public partial class AltaModificacionNacionales : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                Session["Periodistas"] = null;
                Session["Secciones"] = null;

                CargarChkPeriodistas();
                CargarDDLSecciones();
                InhabilitarCalendario();
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                lblMensaje.Text = ex.Detail.InnerText;
            }
            catch (Exception ex)
            {
                lblMensaje.Text = ex.Message;
            }
        }
    }

    private void CargarDDLSecciones()
    {
        ddlSecciones.DataSource = new ServicioEF().ListarSecciones().ToList();
        ddlSecciones.DataValueField = "CodigoSeccion";
        ddlSecciones.DataTextField = "Nombre";
        ddlSecciones.DataBind();
    }
    private void CargarChkPeriodistas()
    {
        chkPeriodistas.DataSource = new ServicioEF().ListarPeriodistas().ToList();
        chkPeriodistas.DataValueField = "Cedula";
        chkPeriodistas.DataTextField = "Nombre";
        chkPeriodistas.DataBind();
    }
    private void DesactivoBotones()
    {
        btnCrear.Enabled = false;
        btnModificar.Enabled = false;
        txtTitulo.Enabled = false;
        txtCuerpo.Enabled = false;
        ddlSecciones.Enabled = false;
        cldFechaPublicacion.Enabled = false;
        ddlImportancia.Enabled = false;
    }

    private void LimpiarCalendario()
    {
        cldFechaPublicacion.VisibleDate = DateTime.Today;
        cldFechaPublicacion.SelectedDate = new DateTime(1970, 1, 1);
    }
    private void LimpioControles()
    {
        txtCodigo.Text = "";
        txtCuerpo.Text = "";
        txtTitulo.Text = "";
        lblMensaje.Text = "";
        InhabilitarCalendario();

        txtCodigo.Enabled = false;
        txtCodigo.ReadOnly = true;
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtCodigo.Text.Length == 0)
                throw new Exception("El codigo de la noticia no puede ser vacio.");

            Noticias _unaNoticia = new ServicioEF().BuscarNoticia(txtCodigo.Text.Trim());

            if (_unaNoticia == null)
            {
                txtCodigo.Enabled = false;
                txtCuerpo.Text = "";
                txtTitulo.Text = "";
                LimpiarCalendario();

                HabilitarCalendario();
                txtTitulo.Enabled = true;
                txtCuerpo.Enabled = true;
                ddlSecciones.Enabled = true;
                ddlImportancia.Enabled = true;
                btnBuscar.Enabled= false;

                lblMensaje.Text = "No hay ninguna una noticia con ese codigo. Puede ingresarla.";

                btnCrear.Enabled = true;
                btnCrear.Visible = true;
                btnModificar.Enabled = false;
            }
            else
            {
                txtCodigo.Text = _unaNoticia.Codigo;
                txtTitulo.Text = _unaNoticia.Titulo;
                txtCuerpo.Text = _unaNoticia.Cuerpo;
                //txtFecha.Text = _unaNoticia.FechaPublicacion.ToString();

                btnCrear.Enabled = false;
                btnModificar.Enabled = true;
                btnModificar.Visible = true;
                Session["Noticia"] = _unaNoticia;
            }
        }
        catch (System.Web.Services.Protocols.SoapException ex)
        {
            lblMensaje.Text = ex.Detail.InnerText;
        }
        catch (Exception ex)
        {
            lblMensaje.Text = ex.Message;
        }
    }

    private void HabilitarCalendario()
    {
        cldFechaPublicacion.Enabled = true;
        cldFechaPublicacion.Visible = true;
    }

    /// <summary>
    /// Inhabilita y limpia el calendario del formulario
    /// </summary>
    private void InhabilitarCalendario()
    {
        cldFechaPublicacion.Enabled = false;
        cldFechaPublicacion.Visible = false;
        LimpiarCalendario();
    }

    protected void btnLimpiar_Click(object sender, EventArgs e)
    {
        try
        {
            Session["Periodista"] = null;
            DesactivoBotones();
            LimpioControles();
            btnBuscar.Enabled = true;
            txtCodigo.Enabled = true;
            txtCodigo.ReadOnly = false;
        }
        catch (Exception ex)
        {
            lblMensaje.Text = ex.Message;
        }
    }

    protected void btnModificar_Click(object sender, EventArgs e)
    {
        ServicioEF servicio = new ServicioEF();
        try
        {
            Noticias _unaN = (Noticias)Session["Noticia"];

            _unaN.Codigo = txtCodigo.Text.Trim();
            _unaN.Secciones.Nombre = ddlSecciones.Text.Trim();
            _unaN.Titulo = txtTitulo.Text.Trim();
            _unaN.Cuerpo = txtCuerpo.Text.Trim();
            _unaN.Importancia = Convert.ToInt32(ddlImportancia.SelectedItem.Value);
            _unaN.FechaPublicacion = cldFechaPublicacion.SelectedDate;
            _unaN.Periodistas = ObtenerPeriodistasSeleccionados(servicio);
            

            servicio.ModificarNoticia(_unaN);

            lblMensaje.Text = "Modificación con Exito";

            txtCodigo.Text = "";
            txtCuerpo.Text = "";
            txtTitulo.Text = "";
            InhabilitarCalendario();

            btnCrear.Enabled = false;
            btnModificar.Enabled = false;
        }
        catch (System.Web.Services.Protocols.SoapException ex)
        {
            lblMensaje.Text = ex.Detail.InnerText;
        }
        catch (Exception ex)
        {
            lblMensaje.Text = ex.Message;
        }
    }

    private Periodistas[] ObtenerPeriodistasSeleccionados(ServicioEF servicio)
    {

        Noticias N = null;
        List<Periodistas> lista = new List<Periodistas>();

        //No funciona correctamente, arma la lista de periodistas pero siempre con el mismo.
        foreach(ListItem item in chkPeriodistas.Items)
        {
            if(item.Selected)
            {
                periodistaSeleccionado = servicio.BuscarPeriodista(item.Value);
                resultado.Add(periodistaSeleccionado);
            }
        }        

        return resultado.ToArray<Periodistas>();
    }

        try
        {
            N = new Noticias()
            {
                Codigo = txtCodigo.Text.Trim(),
                Cuerpo = txtCuerpo.Text.Trim(),
                Titulo = txtTitulo.Text.Trim(),
                FechaPublicacion = cldFechaPublicacion.SelectedDate,
                Importancia = Convert.ToInt32(ddlImportancia.SelectedItem.Value),
                Empleados = (Empleados)Session["usuarioLogueado"],
                Secciones = servicio.BuscarSeccion(ddlSecciones.SelectedItem.Value),
                Periodistas = ObtenerPeriodistasSeleccionados(servicio)
            };
        }
        catch (Exception ex)
        {
            lblMensaje.Text = ex.Message;
            return;
        }

        try
        {
            new ServicioEF().AltaNoticia(N);

            lblMensaje.Text = "Alta con Exito";

            txtCodigo.Text = "";
            txtCuerpo.Text = "";
            txtTitulo.Text = "";
            InhabilitarCalendario();

            btnCrear.Enabled = false;
            btnModificar.Enabled = false;
        }
        catch (System.Web.Services.Protocols.SoapException ex)
        {
            lblMensaje.Text = ex.Detail.InnerText;
        }
        catch (Exception ex)
        {
            lblMensaje.Text = ex.Message;
        }
    }
}
