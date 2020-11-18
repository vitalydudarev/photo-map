using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PhotoMap.Api.Database.Services;
using PhotoMap.Api.DTOs;
using PhotoMap.Api.Hubs;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using ProcessingStatus = PhotoMap.Api.Database.Entities.ProcessingStatus;

namespace PhotoMap.Api.Handlers
{
    public class YandexDiskNotificationHandler : CommandHandler<YandexDiskNotification>
    {
        private readonly YandexDiskHub _yandexDiskHub;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public YandexDiskNotificationHandler(YandexDiskHub yandexDiskHub, IServiceScopeFactory serviceScopeFactory)
        {
            _yandexDiskHub = yandexDiskHub;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is YandexDiskNotification yandexDiskNotification)
            {
                var updateUserDto = new UpdateUserDto
                {
                    YandexDiskStatus = (ProcessingStatus) Enum.Parse(typeof(ProcessingStatus),
                        yandexDiskNotification.Status.ToString())
                };

                using var scope = _serviceScopeFactory.CreateScope();
                var userService = scope.ServiceProvider.GetService<IUserService>();

                await userService.UpdateAsync(yandexDiskNotification.UserId, updateUserDto);

                if (yandexDiskNotification.HasError)
                    await _yandexDiskHub.SendErrorAsync(yandexDiskNotification.UserId, yandexDiskNotification.Message);
            }
        }
    }
}
