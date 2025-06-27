using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using AdminClient.Models;

namespace AdminClient.DTOs
{
    internal class VendingMachineMonitorDTO
    {
        public VendingMachine? vm { get; set; }
        public PaymentMethod[]? pms { get; set; } 
        public MachinePaymentMethod[]? paymentMethods { get; set; }
        public VendingMachineMoney[]? machineMoney { get; set; }
        public SimCard? simCard { get; set; } = new SimCard();
        public Brush? StatusColor { get; set; }
        public decimal? Sum { get; set; }
        public string? basicInfo { get; set; }
        public string? additionalInfo { get; set; }
        public bool? IsCashP { get; set; } = false;
        public bool? IsCardP { get; set; } = false;
        public bool? IsMoneyP { get; set; } = false;
        public bool? IsQR { get; set; } = false;
    }
}
