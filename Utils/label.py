from glob import glob
from scipy.ndimage import label
from PIL import Image
import matplotlib.pyplot as plt
import numpy as np
from random import shuffle


for fname in glob("*.png"):
    if 'mask' in fname: continue
    img = np.asarray(Image.open(fname)) / 255.0

    thr = 0.8

    img[img[..., 3] < 0.1] = 0 
    mask = np.max(img[..., :3], -1) > thr
    labels, num_feats = label(mask)

    for f in range(1, num_feats+1):
        if np.sum(labels == f) < 20:
            labels[labels == f] = 0
    
    labels, num_feats = label(labels != 0)



    labels_shuffle = np.arange(1, num_feats+1)
    shuffle(labels_shuffle)
    labels_shuffle = np.concatenate([[0], labels_shuffle], 0)

    shuffled = 1.0 * labels_shuffle[labels] / num_feats * 256

    labels = shuffled.astype(np.uint8)

    Image.fromarray(labels).save(fname.replace(".png", "_mask.png"))
    
    # plt.imshow(shuffled)
    # plt.show()

