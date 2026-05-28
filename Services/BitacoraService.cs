using System.Text.Json;
using Personal_Sitios.Repositories;

namespace Personal_Sitios.Services
{
    public class BitacoraService
    {
        private readonly BitacoraRepository _repository;

        public BitacoraService(BitacoraRepository repository)
        {
            _repository = repository;
        }

        public void RegistrarConsulta(
            int? idUsuario,
            string entidad)
        {
            _repository.Registrar(
                idUsuario,
                "SELECT",
                entidad,
                $"El usuario consulta {entidad}"
            );
        }

        public void RegistrarInsert(
            int? idUsuario,
            string entidad,
            object datosNuevos)
        {
            _repository.Registrar(
                idUsuario,
                "INSERT",
                entidad,
                $"El usuario registra {entidad}",
                null,
                JsonSerializer.Serialize(datosNuevos)
            );
        }

        public void RegistrarUpdate(
            int? idUsuario,
            string entidad,
            object datosAnteriores,
            object datosNuevos)
        {
            _repository.Registrar(
                idUsuario,
                "UPDATE",
                entidad,
                $"El usuario actualiza {entidad}",
                JsonSerializer.Serialize(datosAnteriores),
                JsonSerializer.Serialize(datosNuevos)
            );
        }

        public void RegistrarDelete(
            int? idUsuario,
            string entidad,
            object datosEliminados)
        {
            _repository.Registrar(
                idUsuario,
                "DELETE",
                entidad,
                $"El usuario elimina {entidad}",
                JsonSerializer.Serialize(datosEliminados),
                null
            );
        }

        public void RegistrarError(
            int? idUsuario,
            string entidad,
            string error)
        {
            _repository.Registrar(
                idUsuario,
                "ERROR",
                entidad,
                error
            );
        }
    }
}