using Microsoft.AspNetCore.Identity;
namespace Projekt.Models
{
    public class UserCart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ICollection<CartItem> CartItems { get; set; }


    }
}
