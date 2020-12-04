import lamport

prkey = lamport.private_key()
pubKey = lamport.public_key(prkey)

sign = lamport.get_sig(msg, prkey)