using BeltExam.Models;
using System.Collections.Generic;

namespace BeltExam.Factory {

    public interface IFactory<T> where T: BaseEntity {}
}