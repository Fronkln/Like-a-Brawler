using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public static class Logic
    {
        public static bool CheckNumberLogicalOperator(double value, double target, LogicalOperator logicOperator)
        {

            switch (logicOperator)
            {
                default:
                    return false;
                case LogicalOperator.TRUE:
                    goto case LogicalOperator.EQUALS;
                case LogicalOperator.FALSE:
                    goto case LogicalOperator.NOT_EQUALS;
                case LogicalOperator.EQUALS:
                    return value == target;
                case LogicalOperator.NOT_EQUALS:
                    return value != target;
                case LogicalOperator.EQUALS_GREATER:
                    return value >= target;
                case LogicalOperator.EQUALS_LESS:
                    return value <= target;
                case LogicalOperator.GREATER:
                    return value > target;
                case LogicalOperator.LESS_THAN:
                    return value < target;
            }
        }
    }
}
