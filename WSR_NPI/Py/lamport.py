import random
import hashlib
def generate_random_256():
    num = random.randint(0,2**256-1)
    bin_num = bin(num)[2:]
    return ''.join(['0' for _ in range(256-len(bin_num))]) + bin_num

def private_key():
    return [to_hex(generate_random_256()) for _ in range(256)], [to_hex(generate_random_256()) for _ in range(256)]

def to_hex(binary):
    val = hex(int(binary,2))[2:]
    return ''.join(['0' for _ in range(64-len(val))]) + val

def to_bin(hexadesimal):
    val = bin(int(hexadesimal,16))[2:]
    return ''.join(['0' for _ in range(256-len(val))]) + val

def public_key(private_key):
    a,b = private_key
    c = [hashlib.sha256(str.encode(to_bin(i))).hexdigest()[2:] for i in a]
    d = [hashlib.sha256(str.encode(to_bin(i))).hexdigest()[2:] for i in b]
    return c,d

def get_sig(massage,private_key):
    msg_hash = hashlib.sha256(str.encode(str(massage))).hexdigest()[2:]
    msg_hash = to_bin(msg_hash)
    sig = []
    for i in range(256):
        if msg_hash[i] == '0':
            sig.append(private_key[0][i])
        else:
            sig.append(private_key[1][i])
    return sig


def varify_sig(massage,sig, public_key):
    temp_sig = get_sig(massage, public_key)
    sig_hash = [hashlib.sha256(str.encode(to_bin(i))).hexdigest()[2:] for i in sig]
    varify = True
    for i in range(256):
        if temp_sig[i] != sig_hash[i] :
            varify = False
    return varify