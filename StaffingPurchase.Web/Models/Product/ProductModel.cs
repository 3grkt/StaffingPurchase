using System;
using FluentValidation.Attributes;
using StaffingPurchase.Web.Validators;

namespace StaffingPurchase.Web.Models.Product
{
    [Validator(typeof(ProductValidator))]
    public class ProductModel : ViewModelBase
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double PV { get; set; }
        public string Sku { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public string NameEn { get; set; }
        public string NetWeight { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
