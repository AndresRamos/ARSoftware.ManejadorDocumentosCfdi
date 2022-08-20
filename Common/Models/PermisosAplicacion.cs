using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public enum PermisosAplicacion
    {
        Ninguno = 0,

        [Display(GroupName = "Permisos",
            Name = "Puede Ver Lista De Solicitudes",
            Description = "El usuario puede ver la lista de solicitudes.")]
        PuedeVerListaSolicitudes,

        [Display(GroupName = "Permisos", Name = "Puede Crear Solicitud", Description = "El usuario puede crear solicitudes.")]
        PuedeCrearSolicitud,

        [Display(GroupName = "Permisos", Name = "Puede Procesar Solicitud", Description = "El usuario puede procesar solicitudes.")]
        PuedeProcesarSolicitud,

        [Display(GroupName = "Permisos", Name = "Puede Editar Usuarios", Description = "El usuario puede editar usuarios.")]
        PuedeEditarUsuarios,

        [Display(GroupName = "Permisos",
            Name = "Puede Editar Configuracion General",
            Description = "El usuario puede editar la configuracion general.")]
        PuedeEditarConfiguracionGeneral,

        [Display(GroupName = "Permisos", Name = "Puede Editar Empresas", Description = "El usuario puede editar las empresas.")]
        PuedeEditarEmpresas,

        [Display(GroupName = "Permisos",
            Name = "Tiene todas las empresas permitidas",
            Description = "El usuario tiene todas las empresas permitidas.")]
        TodasLasEmpresasPermitidas,

        [Display(GroupName = "Permisos", Name = "Todos Los Permisos", Description = "El usuario tiene todos los permisos de aplicacion.")]
        TodosLosPermisos
    }
}
