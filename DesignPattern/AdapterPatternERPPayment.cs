using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Web.Common.AdapterPattern
{
    // ERP system's interface for inventory operations
    public interface IInventorySystem
    {
        void UpdateStock(string productCode, int quantity);
    }

    // External inventory system's API (different from ERP interface)
    public class ExternalInventoryAPI
    {
        public void AdjustStock(string itemCode, int qty)
        {
            // Some logic to update the stock in the external system
        }
    }

    // Adapter class to integrate the external inventory system into the ERP
    public class ExternalInventoryAdapter : IInventorySystem
    {
        private ExternalInventoryAPI externalInventoryAPI;

        public ExternalInventoryAdapter(ExternalInventoryAPI externalInventoryAPI)
        {
            this.externalInventoryAPI = externalInventoryAPI;
        }

        public void UpdateStock(string productCode, int quantity)
        {
            // Convert productCode to itemCode and call the external inventory system method
            externalInventoryAPI.AdjustStock(productCode, quantity);
        }
    }

    // Client code in the ERP system using the inventory system
    public class InventoryClient
    {
        public void AdjustStock(IInventorySystem inventorySystem, string productCode, int quantity)
        {
            inventorySystem.UpdateStock(productCode, quantity);
        }
    }

    class Program
    {
        static void Main()
        {
            // ERP system integrating the external inventory system using the adapter
            InventoryClient inventoryClient = new InventoryClient();
            ExternalInventoryAPI externalInventoryAPI = new ExternalInventoryAPI();
            IInventorySystem externalInventoryAdapter = new ExternalInventoryAdapter(externalInventoryAPI);
            inventoryClient.AdjustStock(externalInventoryAdapter, "P123", 50);
        }
    }
}