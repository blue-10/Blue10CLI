using AutoFixture.Xunit2;
using Blue10CLI.services;
using Blue10SDK;
using Blue10SDK.Models;
using FluentAssertions;
using NSubstitute;
using Objectivity.AutoFixture.XUnit2.AutoNSubstitute.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Blue10CLI.Tests.Services
{
    public class InvoiceServiceTests
    {
        [Theory]
        [AutoMockData]
        public async Task GetNewPostInvoiceAction_Success(
            List<DocumentAction> pDocumentActions,
            [Frozen] IBlue10AsyncClient pBlue10AsyncCLient,
            InvoiceService pInvoiceService)
        {
            // Setup Data
            foreach (var pDocumentAction in pDocumentActions)
            {
                pDocumentAction.Action = EDocumentAction.post_purchase_invoice;
            }

            // Setup services
            pBlue10AsyncCLient.GetDocumentActionsAsync().Returns(pDocumentActions);

            // Get expections
            var fExpectedCount = pDocumentActions.Count;

            // Test
            var fResult = await pInvoiceService.GetNewPostInvoiceAction();

            // Validate
            await pBlue10AsyncCLient.Received(1).GetDocumentActionsAsync();
            fResult.Should().BeOfType<List<DocumentAction>>();
            fResult.Count.Should().Be(fExpectedCount);
        }

        [Theory]
        [AutoMockData]
        public async Task SignInvoice_Success(
             DocumentAction pDocumentAction,
             string pLedgerCode,
             [Frozen] IBlue10AsyncClient pBlue10AsyncCLient,
             InvoiceService pInvoiceService)
        {
            // Setup Data
            pDocumentAction.Status = string.Empty;
            pDocumentAction.PurchaseInvoice.AdministrationCode = string.Empty;
            var fExpected = string.Empty;

            // Setup services
            pBlue10AsyncCLient.EditDocumentActionAsync(Arg.Any<DocumentAction>()).Returns(fExpected);

            // Test
            var fResult = await pInvoiceService.SignInvoice(pDocumentAction, pLedgerCode);

            // Validate
            await pBlue10AsyncCLient.Received(1).EditDocumentActionAsync(
                Arg.Is<DocumentAction>(
                    x => x.Status == "done"
                    && x.PurchaseInvoice.AdministrationCode == pLedgerCode)
                );
            fResult.Should().BeOfType<DocumentAction?>();
            fResult.Should().NotBeNull();
        }
    }
}
