using System;
using System.ComponentModel.DataAnnotations;
using Medical.Core.Enum;

namespace Medical.Service.Dtos.User.OrderDtos
{
	public class OrderCreateDto
	{
        public string Address { get; set; }

        public string Phone { get; set; }

    }
}

