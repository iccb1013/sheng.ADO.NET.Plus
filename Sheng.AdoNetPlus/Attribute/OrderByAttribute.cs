﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.AdoNetPlus
{
    /// <summary>
    /// 在 select 时默认的排序规则
    /// 一个对象中只支持使用一个OrderByAttribute
    /// 多次设置只使用其中一个
    /// </summary>
    public class OrderByAttribute : RelationalMappingAttribute
    {
        private OrderBy _orderBy = OrderBy.ASC;
        public OrderBy OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = value; }
        }

        public OrderByAttribute()
        {

        }

        public OrderByAttribute(OrderBy orderBy)
        {
            this.OrderBy = orderBy;
        }
    }

    public enum OrderBy
    {
        ASC,
        DESC
    }
}
