using System;
using UnityEngine;
public class Bits {

	long[] bits = {0};

	public Bits () {
	}

	public Bits (int nbits) {
		checkCapacity(nbits >> 6);
	}

	public bool get (int index) {
		int word = index >> 6;
		if (word >= bits.Length) return false;
		return (bits[word] & (1L << (index & 0x3F))) != 0L;
	}

	public bool getAndClear (int index) {
		int word = index >> 6;
		if (word >= bits.Length) return false;
		long oldBits = bits[word];
		bits[word] &= ~(1L << (index & 0x3F));
		return bits[word] != oldBits;
	}


	public bool getAndSet (int index) {
		int word = index >> 6;
		checkCapacity(word);
		long oldBits = bits[word];
		bits[word] |= 1L << (index & 0x3F);
		return bits[word] == oldBits;
	}


	public void set (int index) {
		int word = index >> 6;
		checkCapacity(word);
		bits[word] |= 1L << (index & 0x3F);
	}

	public void flip (int index) {
		int word = index >> 6;
		checkCapacity(word);
		bits[word] ^= 1L << (index & 0x3F);
	}

	private void checkCapacity (int len) {
		if (len >= bits.Length) {
			long[] newBits = new long[len + 1];
			Array.Copy(bits, 0, newBits, 0, bits.Length);
			bits = newBits;
		}
	}

	public void clear (int index) {
		int word = index >> 6;
		if (word >= bits.Length) return;
		bits[word] &= ~(1L << (index & 0x3F));
	}

	public void clear () {
		long[] bits = this.bits;
		int length = bits.Length;
		for (int i = 0; i < length; i++) {
			bits[i] = 0L;
		}
	}

	public int numBits () {
		return bits.Length << 6;
	}

	public int length () {
		long[] bits = this.bits;
		for (int word = bits.Length - 1; word >= 0; --word) {
			long bitsAtWord = bits[word];
			if (bitsAtWord != 0) {
				for (int bit = 63; bit >= 0; --bit) {
					if ((bitsAtWord & (1L << (bit & 0x3F))) != 0L) {
						return (word << 6) + bit + 1;
					}
				}
			}
		}
		return 0;
	}

	public bool notEmpty () {
		return !isEmpty();
	}

	public bool isEmpty () {
		long[] bits = this.bits;
		int length = bits.Length;
		for (int i = 0; i < length; i++) {
			if (bits[i] != 0L) {
				return false;
			}
		}
		return true;
	}

	public int nextSetBit (int fromIndex) {
		long[] bits = this.bits;
		int word = fromIndex >> 6;
		int bitsLength = bits.Length;
		if (word >= bitsLength) return -1;
		long bitsAtWord = bits[word];
		if (bitsAtWord != 0) {
			for (int i = fromIndex & 0x3f; i < 64; i++) {
				if ((bitsAtWord & (1L << (i & 0x3F))) != 0L) {
					return (word << 6) + i;
				}
			}
		}
		for (word++; word < bitsLength; word++) {
			if (word != 0) {
				bitsAtWord = bits[word];
				if (bitsAtWord != 0) {
					for (int i = 0; i < 64; i++) {
						if ((bitsAtWord & (1L << (i & 0x3F))) != 0L) {
							return (word << 6) + i;
						}
					}
				}
			}
		}
		return -1;
	}

	public int nextClearBit (int fromIndex) {
		long[] bits = this.bits;
		int word = fromIndex >> 6;
		int bitsLength = bits.Length;
		if (word >= bitsLength) return bits.Length << 6;
		long bitsAtWord = bits[word];
		for (int i = fromIndex & 0x3f; i < 64; i++) {
			if ((bitsAtWord & (1L << (i & 0x3F))) == 0L) {
				return (word << 6) + i;
			}
		}
		for (word++; word < bitsLength; word++) {
			if (word == 0) {
				return word << 6;
			}
			bitsAtWord = bits[word];
			for (int i = 0; i < 64; i++) {
				if ((bitsAtWord & (1L << (i & 0x3F))) == 0L) {
					return (word << 6) + i;
				}
			}
		}
		return bits.Length << 6;
	}

	public void and (Bits other) {
		int commonWords = Mathf.Min(bits.Length, other.bits.Length);
		for (int i = 0; commonWords > i; i++) {
			bits[i] &= other.bits[i];
		}

		if (bits.Length > commonWords) {
			for (int i = commonWords, s = bits.Length; s > i; i++) {
				bits[i] = 0L;
			}
		}
	}

	public void andNot (Bits other) {
		for (int i = 0, j = bits.Length, k = other.bits.Length; i < j && i < k; i++) {
			bits[i] &= ~other.bits[i];
		}
	}

	public void or (Bits other) {
		int commonWords = Mathf.Min(bits.Length, other.bits.Length);
		for (int i = 0; commonWords > i; i++) {
			bits[i] |= other.bits[i];
		}

		if (commonWords < other.bits.Length) {
			checkCapacity(other.bits.Length);
			for (int i = commonWords, s = other.bits.Length; s > i; i++) {
				bits[i] = other.bits[i];
			}
		}
	}

	public void xor (Bits other) {
		int commonWords = Mathf.Min(bits.Length, other.bits.Length);

		for (int i = 0; commonWords > i; i++) {
			bits[i] ^= other.bits[i];
		}

		if (commonWords < other.bits.Length) {
			checkCapacity(other.bits.Length);
			for (int i = commonWords, s = other.bits.Length; s > i; i++) {
				bits[i] = other.bits[i];
			}
		}
	}

	public bool intersects (Bits other) {
		long[] bits = this.bits;
		long[] otherBits = other.bits;
		for (int i = Mathf.Min(bits.Length, otherBits.Length) - 1; i >= 0; i--) {
			if ((bits[i] & otherBits[i]) != 0) {
				return true;
			}
		}
		return false;
	}

	public bool containsAll (Bits other) {
		long[] bits = this.bits;
		long[] otherBits = other.bits;
		int otherBitsLength = otherBits.Length;
		int bitsLength = bits.Length;

		for (int i = bitsLength; i < otherBitsLength; i++) {
			if (otherBits[i] != 0) {
				return false;
			}
		}
		for (int i = Mathf.Min(bitsLength, otherBitsLength) - 1; i >= 0; i--) {
			if ((bits[i] & otherBits[i]) != otherBits[i]) {
				return false;
			}
		}
		return true;
	}


	public override int GetHashCode() {
		int word = length() >> 6;
		int hash = 0;
		for (int i = 0; word >= i; i++) {
			hash = 127 * hash + (int)(bits[i] ^ (bits[i] >> 32));
		}
		return hash;
	}

	public override bool Equals(System.Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (GetType() != obj.GetType())
			return false;

		Bits other = (Bits) obj;
		long[] otherBits = other.bits;

		int commonWords = Mathf.Min(bits.Length, otherBits.Length);
		for (int i = 0; commonWords > i; i++) {
			if (bits[i] != otherBits[i])
				return false;
		}

		if (bits.Length == otherBits.Length)
			return true;

		return length() == other.length();
	}
}
