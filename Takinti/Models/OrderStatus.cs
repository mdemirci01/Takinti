using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Takinti.Models
{
    public enum OrderStatus
    {
        PaymentExpected = 0,
        OrderReviewing = 1,
        OrderApproved = 2,
        OrderPreparing = 3,
        OrderShipping = 4,
        OrderDelivered = 5,
        OrderCancelled = 6
    }
}