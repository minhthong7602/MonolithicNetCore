using System;
using System.IdentityModel.Tokens.Jwt;

namespace MonolithicNetCore.Common.Security.Jwt
{
    public class JwtToken
    {
        private JwtSecurityToken _token;

        public JwtToken(JwtSecurityToken token)
        {
            this._token = token;
        }

        public DateTime ValidTo => _token.ValidTo;
        public string Value => new JwtSecurityTokenHandler().WriteToken(this._token);
    }
}