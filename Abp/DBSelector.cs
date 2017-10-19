using System;
using System.Collections.Generic;
using System.Text;

namespace Abp
{
    //Set the amount of Master/Slave, and I know this setting is so digusting, but it need to do it for the moment. I will optimize this function as a middleware to implement the logic of Master/Slave
    public enum DBSelector
    {
        Master = 100,
        MasterA = 101,
        MasterB = 102,
        MasterC = 103,
        Slave = 150,
        SlaveA = 151,
        SlaveB = 152,
        SlaveC = 153,
        SlaveD = 154,
        SlaveE = 155

    }
}
