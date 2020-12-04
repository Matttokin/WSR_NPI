import sys
sys.path.append(r"F:\Users\MrTokin\Desktop\WSR_NPI\WSR_NPI\lib")
import lamport

prkey = lamport.private_key()
pubKey = lamport.public_key(prkey)

sign = lamport.get_sig(msg, prkey)