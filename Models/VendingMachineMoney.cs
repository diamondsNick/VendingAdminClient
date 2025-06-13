using System.Collections;

namespace AdminClient.Models
{
    public class VendingMachineMoney
    {
        public long ID { get; set; }
        public long VendingMachineID { get; set; }
        public byte Amount { get; set; }
        public VendingMachine VendingMachine { get; set; }
        public Money Money { get; set; }
    }
}
