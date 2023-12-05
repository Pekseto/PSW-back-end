﻿using Explorer.API.Controllers;
using Explorer.API.Controllers.Shopping;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Tests.Integration;

public class WalletTests : BasePaymentsIntegrationTest
{
    public WalletTests(PaymentsTestFactory factory) : base(factory)
    {
    }

    [Fact]
    public void Successfully_created_wallet()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        var controller = CreateController(scope);
        int userId = 1;

        //Act
        var createWalletResponse = ((ObjectResult)controller.CreateWallet(userId).Result).Value as WalletDto;

        // Assert - Response
        createWalletResponse.ShouldNotBeNull();
        createWalletResponse.Id.ShouldBeGreaterThan(0);
        createWalletResponse.UserId.ShouldBe(userId);
        createWalletResponse.Coins.ShouldBe(0);

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedWallet = dbContext.Wallets.FirstOrDefault(x => x.UserId == userId);
        storedWallet.ShouldNotBeNull();
        storedWallet.Id.ShouldBeGreaterThan(0);
        storedWallet.UserId.ShouldBe(userId);
        storedWallet.Coins.ShouldBe(0);
    }

    private static WalletController CreateController(IServiceScope scope)
    {
        return new WalletController(scope.ServiceProvider.GetRequiredService<IWalletService>());
    }
}
