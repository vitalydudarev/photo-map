using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PhotoMap.Api.DTOs;
using PhotoMap.Api.Hubs;
using PhotoMap.Api.Services.Interfaces;
using PhotoMap.Common.Commands;
using PhotoMap.Common.Models;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.Commands;
using ProcessingStatus = PhotoMap.Api.Database.Entities.ProcessingStatus;

namespace PhotoMap.Api.Handlers
{
    public class NotificationHandler : CommandHandler<Notification>
    {
        private readonly YandexDiskHub _yandexDiskHub;
        private readonly DropboxHub _dropboxHub;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public NotificationHandler(
            YandexDiskHub yandexDiskHub,
            DropboxHub dropboxHub,
            IServiceScopeFactory serviceScopeFactory)
        {
            _yandexDiskHub = yandexDiskHub;
            _dropboxHub = dropboxHub;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override async Task HandleAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (command is Notification notification)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var userService = scope.ServiceProvider.GetService<IUserService>();

                var userId = notification.UserIdentifier.UserId;
                var status = (ProcessingStatus) Enum.Parse(typeof(ProcessingStatus),
                    notification.Status.ToString());

                if (notification.UserIdentifier is YandexDiskUserIdentifier)
                {
                    var updateUserDto = new UpdateUserDto { YandexDiskStatus = status };

                    await userService.UpdateAsync(userId, updateUserDto);

                    if (notification.HasError)
                        await _yandexDiskHub.SendErrorAsync(userId, notification.Message);
                }
                else if (notification.UserIdentifier is DropboxUserIdentifier)
                {
                    var updateUserDto = new UpdateUserDto { DropboxStatus = status };

                    await userService.UpdateAsync(userId, updateUserDto);

                    if (notification.HasError)
                        await _dropboxHub.SendErrorAsync(userId, notification.Message);
                }
            }
        }
    }
}
