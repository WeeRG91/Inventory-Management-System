using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IMS.CoreBusiness
{
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = string.Empty;
        [Required]
        public int InventoryId { get; set; }
        [Required]
        public int QuantityBefore { get; set; }
        [Required]
        public int QuantityAfter { get; set; }
        public double UnitPrice { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        [Required]
        public InventoryTransactionType ActivityType { get; set; }
        [Required]
        public string DoneBy { get; set; } = string.Empty;
        
        // Navigation properties
        public Inventory? Inventory { get; set; }
    }
}
