﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModeloEF
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ObligatorioEntities : DbContext
    {
        public ObligatorioEntities()
            : base("name=ObligatorioEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Empleados> Empleados { get; set; }
        public virtual DbSet<Noticias> Noticias { get; set; }
        public virtual DbSet<Periodistas> Periodistas { get; set; }
        public virtual DbSet<Secciones> Secciones { get; set; }
    
        public virtual int AltaEmpleado(string nombreUsuario, string contraseña)
        {
            var nombreUsuarioParameter = nombreUsuario != null ?
                new ObjectParameter("nombreUsuario", nombreUsuario) :
                new ObjectParameter("nombreUsuario", typeof(string));
    
            var contraseñaParameter = contraseña != null ?
                new ObjectParameter("contraseña", contraseña) :
                new ObjectParameter("contraseña", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AltaEmpleado", nombreUsuarioParameter, contraseñaParameter);
        }
    
        public virtual int AltaPeriodista(string cedula, string nombre, string email)
        {
            var cedulaParameter = cedula != null ?
                new ObjectParameter("Cedula", cedula) :
                new ObjectParameter("Cedula", typeof(string));
    
            var nombreParameter = nombre != null ?
                new ObjectParameter("Nombre", nombre) :
                new ObjectParameter("Nombre", typeof(string));
    
            var emailParameter = email != null ?
                new ObjectParameter("Email", email) :
                new ObjectParameter("Email", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AltaPeriodista", cedulaParameter, nombreParameter, emailParameter);
        }
    
        public virtual int AltaSeccion(string codigoSeccion, string nombre)
        {
            var codigoSeccionParameter = codigoSeccion != null ?
                new ObjectParameter("codigoSeccion", codigoSeccion) :
                new ObjectParameter("codigoSeccion", typeof(string));
    
            var nombreParameter = nombre != null ?
                new ObjectParameter("nombre", nombre) :
                new ObjectParameter("nombre", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AltaSeccion", codigoSeccionParameter, nombreParameter);
        }
    
        public virtual int EliminarPeriodista(string cedula, ObjectParameter ret)
        {
            var cedulaParameter = cedula != null ?
                new ObjectParameter("Cedula", cedula) :
                new ObjectParameter("Cedula", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("EliminarPeriodista", cedulaParameter, ret);
        }
    
        public virtual int EliminarSeccion(string codigoSeccion, ObjectParameter ret)
        {
            var codigoSeccionParameter = codigoSeccion != null ?
                new ObjectParameter("CodigoSeccion", codigoSeccion) :
                new ObjectParameter("CodigoSeccion", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("EliminarSeccion", codigoSeccionParameter, ret);
        }
    
        public virtual ObjectResult<Logueo_Result> Logueo(string nombreUsuario, string contraseña)
        {
            var nombreUsuarioParameter = nombreUsuario != null ?
                new ObjectParameter("nombreUsuario", nombreUsuario) :
                new ObjectParameter("nombreUsuario", typeof(string));
    
            var contraseñaParameter = contraseña != null ?
                new ObjectParameter("contraseña", contraseña) :
                new ObjectParameter("contraseña", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Logueo_Result>("Logueo", nombreUsuarioParameter, contraseñaParameter);
        }
    
        public virtual int ModificarPeriodista(string cedula, string nombre, string email)
        {
            var cedulaParameter = cedula != null ?
                new ObjectParameter("Cedula", cedula) :
                new ObjectParameter("Cedula", typeof(string));
    
            var nombreParameter = nombre != null ?
                new ObjectParameter("Nombre", nombre) :
                new ObjectParameter("Nombre", typeof(string));
    
            var emailParameter = email != null ?
                new ObjectParameter("Email", email) :
                new ObjectParameter("Email", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ModificarPeriodista", cedulaParameter, nombreParameter, emailParameter);
        }
    
        public virtual int ModificarSeccion(string codigoSeccion, string nuevoNombre)
        {
            var codigoSeccionParameter = codigoSeccion != null ?
                new ObjectParameter("codigoSeccion", codigoSeccion) :
                new ObjectParameter("codigoSeccion", typeof(string));
    
            var nuevoNombreParameter = nuevoNombre != null ?
                new ObjectParameter("nuevoNombre", nuevoNombre) :
                new ObjectParameter("nuevoNombre", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ModificarSeccion", codigoSeccionParameter, nuevoNombreParameter);
        }
    }
}