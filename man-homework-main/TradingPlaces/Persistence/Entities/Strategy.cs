﻿using Models.Strategy;

namespace Persistence.Entities
{
    public sealed class Strategy
    {
        public string Id { get; set; }
        public string Ticker { get; set; }
        public Instruction Instruction { get; set; }
        public decimal PriceMovement { get; set; }
        public int Quantity { get; set; }
        public decimal ExecutionPrice { get; set; }
    }
}