﻿namespace DreamFactory.Api.Implementation
{
    using System;
    using System.Threading.Tasks;
    using DreamFactory.Http;
    using DreamFactory.Model.User;

    internal partial class UserApi
    {
        public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            if (oldPassword == null)
            {
                throw new ArgumentNullException("oldPassword");
            }

            if (newPassword == null)
            {
                throw new ArgumentNullException("newPassword");
            }

            var address = baseAddress.WithResource("password");
            PasswordRequest data = new PasswordRequest { OldPassword = oldPassword, NewPassword = newPassword };
            string content = contentSerializer.Serialize(data);
            IHttpRequest request = new HttpRequest(HttpMethod.Post, address.Build(), baseHeaders, content);

            IHttpResponse response = await httpFacade.RequestAsync(request);
            HttpUtils.ThrowOnBadStatus(response, contentSerializer);

            return contentSerializer.Deserialize<PasswordResponse>(response.Body).Success ?? false;
        }

        public async Task<PasswordResponse> RequestPasswordResetAsync(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException("email");
            }

            var address = baseAddress.WithResource("password").WithParameter("reset", true);
            PasswordRequest data = new PasswordRequest { Email = email };
            string content = contentSerializer.Serialize(data);
            IHttpRequest request = new HttpRequest(HttpMethod.Post, address.Build(), baseHeaders, content);

            IHttpResponse response = await httpFacade.RequestAsync(request);
            HttpUtils.ThrowOnBadStatus(response, contentSerializer);

            return contentSerializer.Deserialize<PasswordResponse>(response.Body);
        }

        public async Task<bool> CompletePasswordResetAsync(string email, string newPassword, string code, string answer)
        {
            if (email == null)
            {
                throw new ArgumentNullException("email");
            }

            if (newPassword == null)
            {
                throw new ArgumentNullException("newPassword");
            }

            if (code != null && answer != null)
            {
                throw new ArgumentException("You must specify either code or answer parameters but not both.", "answer");
            }

            var address = baseAddress.WithResource("password").WithParameter("login", true);
            PasswordRequest data = new PasswordRequest { Email = email, NewPassword = newPassword, Code = code, SecurityAnswer = answer };
            string content = contentSerializer.Serialize(data);
            IHttpRequest request = new HttpRequest(HttpMethod.Post, address.Build(), baseHeaders, content);

            IHttpResponse response = await httpFacade.RequestAsync(request);
            HttpUtils.ThrowOnBadStatus(response, contentSerializer);

            bool success = contentSerializer.Deserialize<PasswordResponse>(response.Body).Success ?? false;
            if (success)
            {
                Session session = await GetSessionAsync();
                baseHeaders.AddOrUpdate(HttpHeaders.DreamFactorySessionTokenHeader, session.SessionId);
            }

            return success;
        }
    }
}
