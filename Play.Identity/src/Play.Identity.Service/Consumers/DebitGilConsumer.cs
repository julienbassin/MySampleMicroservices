using MassTransit;
using Microsoft.AspNetCore.Identity;
using Play.Identity.Service.Entities;
using Play.Identity.Service.Exceptions;
using System.Threading.Tasks;
using static Play.Identity.Contracts.Contracts;

namespace Play.Identity.Service.Consumers
{
    public class DebitGilConsumer : IConsumer<DebitGil>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public DebitGilConsumer(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task Consume(ConsumeContext<DebitGil> context)
        {
            var message = context.Message;
            var user = await userManager.FindByIdAsync(message.userId.ToString());
            if (user == null) 
            {
                throw new UnkwnownUserException(message.userId);
            }

            user.Gil -= message.Gil;

            if (user.Gil < 0)
            {
                throw new InsufficientFundsException(message.userId, message.Gil);
            }

            await userManager.UpdateAsync(user);
            await context.Publish(new GilDebited(message.CorrelationId);
        }
    }
}
