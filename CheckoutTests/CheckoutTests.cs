﻿using CheckoutBL;
using NUnit.Framework;
using System;
using System.Linq;

namespace CheckoutTests
{
    [TestFixture]
    public class CheckoutTests
    {
        private Checkout _classUnderTest;
        private readonly Product productA = new Product { SKU = 'A', UnitPrice = 50 };
        private readonly Product productB = new Product { SKU = 'B', UnitPrice = 30 };
        private readonly Product emptyProduct = null;
        private readonly SpecialOffer offerA = new SpecialOffer { SKU = 'A', OfferQuantity = 3, SpecialPrice = 130 };
        private readonly SpecialOffer offerB = new SpecialOffer { SKU = 'B', OfferQuantity = 2, SpecialPrice = 45 };
        private readonly SpecialOffer emptyOffer = null;


        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new Checkout();
        }

        [Test]
        public void Checkout_ScanProduct_Should_AddProductToList()
        {
            _classUnderTest.ScanProduct(productA);

            Assert.AreEqual(1, _classUnderTest.products.Count());
            Assert.AreEqual('A', _classUnderTest.products[0].SKU);
        }

        [Test]
        public void Checkout_ScanProduct_Should_ThrowExceptionIfProductNull()
        {
           Assert.Throws<ArgumentNullException>(() => _classUnderTest.ScanProduct(emptyProduct));            
        }

        [Test]
        public void Checkout_ScanProduct_Should_AddNumberOfItemsScannedToList()
        {
            for (int i = 1; i < 5; i++)
            {
                _classUnderTest.ScanProduct(productA);
            }

            Assert.AreEqual(4, _classUnderTest.products.Count());
        }

        [Test]
        public void Checkout_AddSpecialOfferRule_Should_AddOfferToList()
        {
            _classUnderTest.AddSpecialOfferRule(offerA);

            Assert.AreEqual(1, _classUnderTest.offers.Count());
            Assert.AreEqual('A', _classUnderTest.offers[0].SKU);
        }

        [Test]
        public void Checkout_AddSpecialOfferRule_Should_ThrowExceptionIfOfferRuleNull()
        {
            Assert.Throws<ArgumentNullException>(() => _classUnderTest.AddSpecialOfferRule(emptyOffer));
        }

        [Test]
        public void Checkout_GetTotalPrice_Should_ReturnZeroIfNoProductScanned()
        {
            var result = _classUnderTest.GetTotalPrice();

            Assert.AreEqual(0, result);
        }

        [Test]
        public void Checkout_GetTotalPrice_Should_ReturnUnitPriceOfProductA_IfProductAScanned()
        {
            _classUnderTest.ScanProduct(productA);

            var result = _classUnderTest.GetTotalPrice();

            Assert.AreEqual(50, result);
        }

        [Test]
        public void Checkout_GetTotalPrice_Should_ReturnSpecialOfferIfValidProducts()
        {
            _classUnderTest.AddSpecialOfferRule(offerA);

            for (int i = 1; i < 4; i++)
            {
                _classUnderTest.ScanProduct(productA);
            }

            var result = _classUnderTest.GetTotalPrice();

            Assert.AreEqual(130, result);
        }

        [Test]
        public void Checkout_GetTotalPrice_Should_ReturnSpecialOfferAndUnitPriceTotal()
        {
            _classUnderTest.AddSpecialOfferRule(offerA);

            // one offer price and one unit price, product a
            for (int i = 1; i < 5; i++)
            {
                _classUnderTest.ScanProduct(productA);
            }

            var result = _classUnderTest.GetTotalPrice();

            Assert.AreEqual(180, result);
        }

        [Test]
        public void Checkout_GetTotalPrice_Should_ReturnSpecialOfferPricingIfMultipleSpecialOffers()
        {
            _classUnderTest.AddSpecialOfferRule(offerA);
            _classUnderTest.AddSpecialOfferRule(offerB);

            // product b special offer (2 items)
            for (int i = 1; i < 3; i++)
            {
                _classUnderTest.ScanProduct(productB);
            }
            // product a special offer (3 items)
            for (int i = 1; i < 4; i++)
            {
                _classUnderTest.ScanProduct(productA);
            }

            var result = _classUnderTest.GetTotalPrice();

            Assert.AreEqual(175, result);
        }        
    }
}
