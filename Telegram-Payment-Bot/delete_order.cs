using System.Runtime.Remoting.Messaging;
using System.Security.Policy;

import requests

def delete_sellix_order(api_key, uniqid):
    url = f"https://dev.sellix.io/v1/payments/{uniqid}"
    headers = {
    "Authorization": f"Bearer {api_key}"
    }

response = requests.delete(url, headers = headers)
    if response.status_code == 200:
        return True, f"Order {uniqid} has been deleted successfully."
    else:
        return False, f"Failed to delete order {uniqid}: {response.text}"
