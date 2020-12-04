import lamport
import sys
sys.path.append("F:\\Users\\MrTokin\\Desktop\\WSR_NPI\\WSR_NPI\\Py")

prkey = lamport.private_key()
pubKey = lamport.public_key(prkey)

sign = lamport.get_sig(msg, prkey)