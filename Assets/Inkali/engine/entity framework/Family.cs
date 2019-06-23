using System;
using System.Collections.Generic;
using System.Text;

public class Family
{
    private static Dictionary<string, Family> families = new Dictionary<string, Family>();
    private static int familyIndex = 0;
    private static  Builder builder = new Builder();
    private static  Bits zeroBits = new Bits();

    private  Bits _all;
	private  Bits _one;
	private  Bits _exclude;
	private  int index;

    /** Private constructor, use static method Family.getFamilyFor() */
    private Family(Bits all, Bits any, Bits exclude)
    {
        this._all = all;
        this._one = any;
        this._exclude = exclude;
        this.index = familyIndex++;
    }

    /** @return This family's unique index */
    public int getIndex()
    {
        return this.index;
    }

    /** @return Whether the entity matches the family requirements or not */
    public bool matches(Entity entity)
    {
        Bits entityComponentBits = entity.getComponentBits();

        if (!entityComponentBits.containsAll(_all))
        {
            return false;
        }

        if (!_one.isEmpty() && !_one.intersects(entityComponentBits))
        {
            return false;
        }

        if (!_exclude.isEmpty() && _exclude.intersects(entityComponentBits))
        {
            return false;
        }

        return true;
    }

    /**
	 * @param componentTypes entities will have to contain all of the specified components.
	 * @return A Builder singleton instance to get a family
	 */
    public static  Builder all(params Type[] componentTypes)
    {
        return builder.reset().all(componentTypes);
    }

    /**
	 * @param componentTypes entities will have to contain at least one of the specified components.
	 * @return A Builder singleton instance to get a family
	 */
    public static  Builder one(params Type[] componentTypes)
    {
        return builder.reset().one(componentTypes);
    }

    /**
	 * @param componentTypes entities cannot contain any of the specified components.
	 * @return A Builder singleton instance to get a family
	 */
    public static  Builder exclude(params Type[] componentTypes)
    {
        return builder.reset().exclude(componentTypes);
    }

    public class Builder
    {
        private Bits _all = zeroBits;
        private Bits _one = zeroBits;
        private Bits _exclude = zeroBits;

        public Builder()
        {

        }

        /**
		 * Resets the builder instance
		 * @return A Builder singleton instance to get a family
		 */
        public Builder reset()
        {
            _all = zeroBits;
            _one = zeroBits;
            _exclude = zeroBits;
            return this;
        }

        /**
		 * @param componentTypes entities will have to contain all of the specified components.
		 * @return A Builder singleton instance to get a family
		 */
        public  Builder all(params Type[] componentTypes)
        {
            _all = ComponentType.getBitsFor(componentTypes);
            return this;
        }

        /**
		 * @param componentTypes entities will have to contain at least one of the specified components.
		 * @return A Builder singleton instance to get a family
		 */
        public  Builder one(params Type[] componentTypes)
        {
            _one = ComponentType.getBitsFor(componentTypes);
            return this;
        }

        /**
		 * @param componentTypes entities cannot contain any of the specified components.
		 * @return A Builder singleton instance to get a family
		 */
        public  Builder exclude(params Type[] componentTypes)
        {
            _exclude = ComponentType.getBitsFor(componentTypes);
            return this;
        }

        /** @return A family for the configured component types */
        public Family get()
        {
            string hash = getFamilyHash(_all, _one, _exclude);
            Family family = null;
            if(families.ContainsKey(hash))
                family = families[hash];
            if (family == null)
            {
                family = new Family(_all, _one, _exclude);
                families.Add(hash, family);
            }
            return family;
        }
    }

    public override int GetHashCode()
    {
        return index;
    }
    public override bool Equals(object obj)
    {
        return this == obj;
    }

    private static string getFamilyHash(Bits all, Bits one, Bits exclude)
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (!all.isEmpty())
        {
            stringBuilder.Append("{all:").Append(getBitsString(all)).Append("}");
        }
        if (!one.isEmpty())
        {
            stringBuilder.Append("{one:").Append(getBitsString(one)).Append("}");
        }
        if (!exclude.isEmpty())
        {
            stringBuilder.Append("{exclude:").Append(getBitsString(exclude)).Append("}");
        }
        return stringBuilder.ToString();
    }

    private static string getBitsString(Bits bits)
    {
        StringBuilder stringBuilder = new StringBuilder();

        int numBits = bits.length();
        for (int i = 0; i < numBits; ++i)
        {
            stringBuilder.Append(bits.get(i) ? "1" : "0");
        }

        return stringBuilder.ToString();
    }
}