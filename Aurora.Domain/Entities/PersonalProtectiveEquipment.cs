using Flunt.Validations;
using System;
using System.Collections.Generic;

namespace Aurora.Domain.Entities
{
    public class PersonalProtectiveEquipment : BaseEntity<int>
    {
        public PersonalProtectiveEquipment(string description, int quantity, string approvalCertificate, DateTime manufacturingDate, int durability)
        {
            AddNotifications(
                ValidateDescription(description),
                ValidateQuantity(quantity),
                ValidateApprovalCertificate(approvalCertificate),
                ValidateManufacturingDate(manufacturingDate),
                ValidateDurability(durability));

            if (Valid)
            {
                Description = description;
                Quantity = quantity;
                ApprovalCertificate = approvalCertificate;
                ManufacturingDate = manufacturingDate;
                Durability = durability;
            }
        }

        protected PersonalProtectiveEquipment() { }

        public string Description { get; }

        public int Quantity { get; }

        public string ApprovalCertificate { get; }

        public DateTime ManufacturingDate { get; }

        public int Durability { get; }

        public virtual IEnumerable<PpePossession> PpePossessions { get; set; }

        public bool HasExpired() =>
            ManufacturingDate.AddDays(Durability) < DateTime.Now;

        private Contract ValidateDescription(string description) =>
            new Contract()
                .HasMinLen(description, 10, nameof(Description), "A descrição precisa ter mais do que 10 caracteres")
                .IsNotNullOrWhiteSpace(description, nameof(Description), "È necessário informar uma descrição");

        private Contract ValidateQuantity(int quantity) =>
            new Contract()
                .IsLowerOrEqualsThan(quantity, 0, nameof(Quantity), "A quantidade precisa ser mair do que  0.");

        private Contract ValidateApprovalCertificate(string approvalCertificate) =>
            new Contract()
                .IsNotNullOrWhiteSpace(approvalCertificate, nameof(ApprovalCertificate), "É necessário informar um certificado de aprovação.");

        private Contract ValidateManufacturingDate(DateTime manufacturingDate) =>
            new Contract()
                .IsGreaterThan(manufacturingDate, DateTime.Now, nameof(ManufacturingDate), "A data de execução não pode ser maior do que a de hoje");

        private Contract ValidateDurability(int durability) =>
            new Contract()
                .IsLowerOrEqualsThan(durability, 0, nameof(Durability), "A durabilidade precisa ser mais do que 0 dias");
    }
}
