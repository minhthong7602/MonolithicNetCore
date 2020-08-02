using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace MonolithicNetCore.Common.Security.Jwt
{
    public class JwtTokenBuilder
    {
        #region Fields

        private SecurityKey _securityKey = null;
        private string _subject = "";
        private string _issuer = "";
        private string _audience = "";
        private Dictionary<string, string> _claims = new Dictionary<string, string>();
        private int _expiryInMinutes = 5;

        #endregion Fields

        #region Method

        /// <summary>
        /// Add security key
        /// </summary>
        /// <param name="securityKey"></param>
        /// <returns>Self</returns>
        public JwtTokenBuilder AddSecurityKey(SecurityKey securityKey)
        {
            this._securityKey = securityKey;
            return this;
        }

        /// <summary>
        /// Add subject
        /// </summary>
        /// <param name="subject"></param>
        /// <returns>Self</returns>
        public JwtTokenBuilder AddSubject(string subject)
        {
            this._subject = subject;
            return this;
        }

        /// <summary>
        /// Add Issuer
        /// </summary>
        /// <param name="issuer"></param>
        /// <returns>Self</returns>
        public JwtTokenBuilder AddIssuer(string issuer)
        {
            this._issuer = issuer;
            return this;
        }

        /// <summary>
        /// Add auience
        /// </summary>
        /// <param name="audience"></param>
        /// <returns>Self</returns>
        public JwtTokenBuilder AddAudience(string audience)
        {
            this._audience = audience;
            return this;
        }

        /// <summary>
        /// Add dictionary claims
        /// </summary>
        /// <param name="claims"></param>
        /// <returns>Self</returns>
        public JwtTokenBuilder AddClaims(Dictionary<string, string> claims)
        {
            this._claims.Union(claims);
            return this;
        }

        /// <summary>
        /// Add a claim
        /// </summary>
        /// <param name="type">type of claim</param>
        /// <param name="value">vaule of claim</param>
        /// <returns>Self</returns>
        public JwtTokenBuilder AddClaim(string type, string value)
        {
            this._claims.Add(type, value);
            return this;
        }

        /// <summary>
        /// Add expiry time
        /// </summary>
        /// <param name="expiryInMinutes"></param>
        /// <returns>Self</returns>
        public JwtTokenBuilder AddExpiryInMinutes(int expiryInMinutes)
        {
            this._expiryInMinutes = expiryInMinutes;
            return this;
        }

        /// <summary>
        /// Check null exception of secritykey, subject, issuer, audience
        /// </summary>
        private void EnsureArguments()
        {
            if (this._securityKey == null)
                throw new ArgumentNullException("Security Key");

            if (string.IsNullOrEmpty(this._subject))
                throw new ArgumentNullException("Subject");

            if (string.IsNullOrEmpty(this._issuer))
                throw new ArgumentNullException("Issuer");

            if (string.IsNullOrEmpty(this._audience))
                throw new ArgumentNullException("Audience");
        }

        /// <summary>
        /// Build Jwt token
        /// </summary>
        /// <returns>JwtToken</returns>
        public JwtToken Build()
        {
            var claims = new List<Claim>
            {
              new Claim(JwtRegisteredClaimNames.Sub, this._subject),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }
           .Union(this._claims.Select(item => new Claim(item.Key, item.Value)));

            var token = new JwtSecurityToken
            (
                issuer: this._issuer,
                claims: claims,
                audience: this._audience,
                expires: DateTime.UtcNow.AddMinutes(this._expiryInMinutes),
                signingCredentials: new SigningCredentials(this._securityKey, SecurityAlgorithms.HmacSha256)
            );
            return new JwtToken(token);
        }

        public static JwtSecurityToken DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }

        #endregion Method
    }
}