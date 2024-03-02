﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.AuthService.Models;
using OnlineStore.CommonComponent.EventModels.UserModels;
using OnlineStore.CommonComponent.Kafka.Services;

namespace OnlineStore.AuthService.Core.Handlers.ConfirmEmail;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailRequest, ConfirmEmailResponse>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IServiceProvider serviceProvider;
    private readonly ProduserService produserService;

    public ConfirmEmailHandler(IServiceProvider serviceProvider, ProduserService produserService)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        var serviceScope = this.serviceProvider.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope();
        _userManager = serviceScope.ServiceProvider.GetService<UserManager<AuthUser>>();

        this.produserService = produserService ?? throw new ArgumentNullException();
    }

    public async Task<ConfirmEmailResponse> Handle(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new Exception("User does not exists!");
        var result = await _userManager.ConfirmEmailAsync(user, request.ConfirmationToken);

        EmailConfermedEvent emailConfermedEvent = new EmailConfermedEvent()
        {
            Email = request.Email,
            UserId = Guid.Parse(user.Id)
        };

        if (result.Succeeded)
        {
            await this.produserService.ProduceAsync(emailConfermedEvent);
        }

        return new ConfirmEmailResponse(result.Succeeded);
    }
}