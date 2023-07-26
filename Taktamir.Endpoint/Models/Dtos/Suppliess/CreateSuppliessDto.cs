using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Taktamir.Endpoint.Models.Dtos.Suppliess
{
    public class CreateSuppliessDto
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }
    public class SuppliessDto
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
