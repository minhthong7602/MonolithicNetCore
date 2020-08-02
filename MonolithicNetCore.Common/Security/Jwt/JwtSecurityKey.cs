using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MonolithicNetCore.Common.Security.Jwt
{
    public class JwtSecurityKey
    {
        /// <summary>
        /// Create Symmetric Security Key
        /// </summary>
        /// <param name="secretKey">Secret Key been config in app setting json file</param>
        /// <returns></returns>
        public static SymmetricSecurityKey Create(string secretKey)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
        }
    }
}