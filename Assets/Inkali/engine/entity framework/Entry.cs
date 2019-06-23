using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Entry<K, V>
{
    public K key;
    public V value;

    public String toString()
    {
        return key + "=" + value;
    }
}