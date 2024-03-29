﻿using System.ComponentModel;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.WebApplication.ViewModels.Finance
{
    public class OrderViewModel
    {
        [DisplayName("Баланс пользователя")]
        public int UserBalance { get; set; }

        [DisplayName("Общая стоимость заказа")]
        public int OrderPrice { get; set; }

        public int ContentPackageId { get; set; }
    }
}