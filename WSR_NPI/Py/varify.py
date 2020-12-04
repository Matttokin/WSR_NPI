import sys
sys.path.append(r"F:\Users\MrTokin\Desktop\WSR_NPI\WSR_NPI\lib")
import lamport

result = lamport.varify_sig(msg, sign, pubKey)