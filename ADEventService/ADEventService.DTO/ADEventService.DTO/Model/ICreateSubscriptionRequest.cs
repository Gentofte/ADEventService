﻿using System;
namespace ADEventService.DTO
{
    // ================================================================================
    public interface ICreateSubscriptionRequest
    {
        // -----------------------------------------------------------------------------
        Guid ID { get; set; }
        // -----------------------------------------------------------------------------
        string Name { get; set; }
        // -----------------------------------------------------------------------------
        string Description { get; set; }
        // -----------------------------------------------------------------------------
        string Endpoint { get; set; }
        // -----------------------------------------------------------------------------
        string ContactEmail { get; set; }
    }
}