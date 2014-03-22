import javax.swing.BoxLayout;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JTextArea;
import javax.swing.JTextField;

/**
 * 
 * @author Aurion / aurion@gmail.com
 * 
 */
public class JOptionPaneMultiInput extends JFrame {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	static Double[] results;

	public JOptionPaneMultiInput(String[] labels, JTextArea messages) {
		this.setAlwaysOnTop(true);
		this.requestFocus();
		JPanel myPanel = new JPanel();
		BoxLayout boxLayout = new BoxLayout(myPanel, BoxLayout.Y_AXIS);
		myPanel.setLayout(boxLayout);
		myPanel.add(new JLabel("Please enter an average marketplace price for one of each"));
		myPanel.add(new JLabel("		"));
		for (String label : labels) {
			JTextField textField = new JTextField(5);
			myPanel.add(new JLabel(label));
			myPanel.add(textField);
		}
		int j = 0;
		int position = 0;
		JOptionPaneMultiInput.results = new Double[myPanel.getComponentCount()];
		int result = JOptionPane.showConfirmDialog(null, myPanel,
				"Material cost", JOptionPane.OK_CANCEL_OPTION);
		if (result == JOptionPane.OK_OPTION) {
			for (int i = 3; i < myPanel.getComponentCount(); i = i + 2) {
				JTextField costField = (JTextField) myPanel.getComponent(i);
				try {
					JOptionPaneMultiInput.results[j] = Double.parseDouble(costField.getText());
					position++;
				} catch (NumberFormatException e) {
					messages.setText("You need to enter a valid number for "
							+ "\n"+ labels[position].substring(0,labels[position].indexOf(" x")) + ". Please try again");
				}
				j++;
			}
		}
	}

}
