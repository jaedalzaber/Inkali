using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface Poolable
{
    /** Resets the object for reuse. Object references should be nulled and fields may be set to default values. */
    void reset();
}