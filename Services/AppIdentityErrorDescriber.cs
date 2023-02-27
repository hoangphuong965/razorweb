using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Services
{
    public class AppIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateRoleName(string role)
        {
            var er = base.DuplicateRoleName(role);

            return new IdentityError()
            {
                Code = er.Code,
                Description = $"Tên role {role} đã được sử dụng."
            };
        }
    }
}
