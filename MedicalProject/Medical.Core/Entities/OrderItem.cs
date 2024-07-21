﻿using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medical.Core.Entities
{
	public class OrderItem:BaseEntity
	{
        public int Count { get; set; }

        public int OrderId { get; set; }

        public int MedicineId { get; set; }

          public double Price { get; set; }

        public Order? Order { get; set; }

        public Medicine?  Medicine { get; set; }

    }
}
