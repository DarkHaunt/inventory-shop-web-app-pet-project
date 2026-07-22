using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InventoryShop.Infrastructure.Authentication;

public sealed class JwtSecurityTokenProvider(IOptions<JwtOptions> options) : ISecurityTokenProvider
{
   public string GenerateSecurityTokenFor(PlayerEntity player)
   {
      var claims = new List<Claim>()
      {
         new(ClaimTypes.NameIdentifier, player.Id.ToString()),
         new(ClaimTypes.Name, player.Nickname),
      };
      
      var signingCredentials = new SigningCredentials(
         new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)), SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
         signingCredentials: signingCredentials,
         claims: claims,
         expires: DateTime.Now.AddHours(options.Value.ExpirationHours)
      );
      
      return new JwtSecurityTokenHandler().WriteToken(token);
   }
}